/*

DROP DATABASE [WidgetDB]
GO

CREATE DATABASE [WidgetDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'WidgetDB', FILENAME = N'D:\SQLDB\MSSQL12.MSSQLSERVER\MSSQL\DATA\WidgetDB.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'WidgetDB_log', FILENAME = N'D:\SQLDB\MSSQL12.MSSQLSERVER\MSSQL\DATA\WidgetDB_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

*/
use WidgetDB
GO

if object_id('supplier_part_assoc') is not null drop table supplier_part_assoc
if object_id('widget_part_assoc') is not null drop table widget_part_assoc
if object_id('widget') is not null drop table widget
if object_id('part') is not null drop table part
if object_id('supplier') is not null drop table supplier

create table widget
(
	widget_id int identity(1, 1) not null,
	widget_name varchar(100) not null,
	constraint PK_widget primary key clustered (widget_id)
)
GO

create table part
(
	part_id int identity(1,1) not null,
	part_name varchar(100) not null,
	cost decimal(8, 2) not null,
	constraint PK_part
		primary key clustered (part_id),
	constraint unq_part_name
		unique (part_name),
	constraint chk_cost
		check (cost > 0)
)
GO

create unique index ndx_part_part_name on part(part_name)
go

create table widget_part_assoc
(
	-- PK Fields
	widget_id int not null,							-- Id of the widget containing the part
	part_id int not null,							-- Id of the part used
	blueprint_id int not null,						-- Id on the blueprint for the widget where the part is installed.

	build_labor_cost numeric(8,2) not null,			-- Initially manufactoring labor cost  
	replacement_labor_cost numeric(8,2) not null,	-- Labor cost to replace the part

	constraint pk_widget_part_assoc
		primary key clustered (widget_id, part_id, blueprint_id),
	constraint fk_widget_part_assoc_widget
		foreign key (widget_id)
		references widget (widget_id),
	constraint fk_widget_part_assoc_part
		foreign key (part_id)
		references part (part_id)
)
go

create table supplier
(
	supplier_id int identity (1,1) not null,
	name varchar(100) not null,
	preferred_vendor bit not null default(0),
	constraint pk_supplier
		primary key clustered (supplier_id)
)
go

create table supplier_part_assoc
(
	supplier_id int not null,
	part_id int not null,
	last_wholesale_price numeric(8,2) not null,
	last_order_quantity int not null,
	constraint pk_supplier_part_assoc
		primary key clustered (supplier_id, part_id),
	constraint fk_supplier_part_assoc_supplier
		foreign key (supplier_id)
		references supplier (supplier_id),
	constraint fk_supplier_part_assoc_part
		foreign key (part_id)
		references part (part_id)
)
go

insert into widget (widget_name)
values
('Thingamabob'), ('Thingamajig'), ('Thingy'),
('Doomaflotchie'), ('Doohickey'), ('Doojigger'), ('Doodad'),
('Whatchamacallit'), ('Whatnot'), ('Whatsit'),
('Gizmo'), ('Nicknack')
GO

insert into part (part_name, cost)
values
('3/4 inch screw', .24),
('1/2 inch screw', .16),
('1/4 inch screw', .08),
('panel, large', 3.75),
('panel, medium', 2.50),
('panel, small', 1.25),
('spring, large ', 3.15),
('spring, medium', 2.10),
('spring, small', 1.05),
('cog, large', 6.60),
('cog, medium', 4.40),
('cog, small', 2.20),
('gear, large', 37.02),
('gear, medium', 24.68),
('gear, small', 12.34),
('axle, large', 6.99),
('axle, medium', 4.66),
('axle, small', 2.33),
('flywheel, large', 44.44),
('flywheel, medium', 33.33),
('flywheel, small', 22.22),
('valve, large', 0.40),
('valve, medium', 0.30),
('valve, small', 0.20),
('spigot, large', 3.40),
('spigot, medium', 2.30),
('spigot, small', 1.20),
('camshaft, large', 183.64),
('camshaft, medium', 122.42),
('camshaft, small', 61.21),
('sprocket, large', 63.36),
('sprocket, medium', 42.24),
('sprocket, small', 21.12),
('switch, large', 63.36),
('switch, medium', 42.24),
('switch, small', 21.12)
go

insert into supplier
(name, preferred_vendor)
values
('Discount Parts, Inc.', 1),
('Parts-R-Us, LLP.', 0),
('Builder''s Bazaar', 0),
('American Standard Parts Corp.', 1),
('Parts-Mart', 0)
go

set nocount on
-- CREATE SOME RANDOM DATA FOR [widget_part_assoc]

declare widget_cursor cursor fast_forward for
select widget_id from widget with (nolock)

declare @widget_id int, @part_id int, @parts_in_widget int, @total_parts_available int, @blueprint_id int
declare @build_labor_cost numeric(8,2), @replacement_labor_cost numeric(8,2)

select @total_parts_available = count(part_id) from part with (nolock)

open widget_cursor
fetch next from widget_cursor into @widget_id

while @@fetch_status = 0
begin
	-- Give each widget between 50 and 100 parts
	set @parts_in_widget = (ABS(CAST(CRYPT_GEN_RANDOM(8) AS bigint)) % 50) + 50
	set @blueprint_id = 0

	while @parts_in_widget > 0
	begin
		set @part_id = (ABS(CAST(CRYPT_GEN_RANDOM(8) AS bigint)) % @total_parts_available) + 1
		set @blueprint_id = @blueprint_id + 1
		set @build_labor_cost = ABS(CAST(CRYPT_GEN_RANDOM(8) AS bigint)) % 10000 / 100.00
		set @replacement_labor_cost = ABS(CAST(CRYPT_GEN_RANDOM(8) AS bigint)) % 10000 / 100.00
		
		--PRINT 'WIDGET: ' + cast(@widget_id as varchar) + ', PART: '+ cast(@part_id as varchar) + ', BLUEPRINT_ID: ' + cast(@blueprint_id as varchar) 
		insert into widget_part_assoc(widget_id, part_id, blueprint_id, build_labor_cost, replacement_labor_cost)
		values(@widget_id, @part_id, @blueprint_id, @build_labor_cost, @replacement_labor_cost)

		set @parts_in_widget = @parts_in_widget - 1
	end

	fetch next from widget_cursor into @widget_id
end

close widget_cursor
deallocate widget_cursor


-- CREATE SOME RANDOM DATA FOR [spplier_part_assoc]
declare supplier_cursor cursor fast_forward for
select supplier_id from supplier with (nolock)

declare @supplier_id int
declare @last_wholesale_price numeric(8,2), @build_cost numeric(8,2), @markup_percentage numeric(4, 4)
declare @last_order_quantity int

declare part_cursor cursor fast_forward for
select part_id, cost from part with (nolock)

open supplier_cursor
fetch next from supplier_cursor into @supplier_id

while @@fetch_status = 0
begin
	open part_cursor
	fetch next from part_cursor into @part_id, @build_cost

	while @@fetch_status = 0
	begin
		if CAST(CRYPT_GEN_RANDOM(8) AS bigint) > 0
		BEGIN
			set @markup_percentage = ABS(CAST(CRYPT_GEN_RANDOM(8) AS bigint)) % 5000 / 10000.00
			set @last_order_quantity = ABS(CAST(CRYPT_GEN_RANDOM(8) AS bigint)) % 1000

			insert into supplier_part_assoc(supplier_id, part_id, last_wholesale_price, last_order_quantity)
			values (@supplier_id, @part_id, @build_cost / (1.0 + @markup_percentage), @last_order_quantity)
		END

		fetch next from part_cursor into @part_id, @build_cost
	END

	close part_cursor
	fetch next from supplier_cursor into @supplier_id
end

deallocate part_cursor
set nocount off

--Scaffold-DbContext -Connection "Server=localhost;Database=WidgetDB;Trusted_Connection=True;" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models