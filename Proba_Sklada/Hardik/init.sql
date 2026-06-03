-- DROP SCHEMA sklad;

CREATE SCHEMA sklad AUTHORIZATION "user";

-- DROP SEQUENCE sklad.counterparties_id_seq;

CREATE SEQUENCE sklad.counterparties_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.expense_invoice_items_id_seq;

CREATE SEQUENCE sklad.expense_invoice_items_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.expense_invoices_id_seq;

CREATE SEQUENCE sklad.expense_invoices_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.income_invoice_items_id_seq;

CREATE SEQUENCE sklad.income_invoice_items_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.income_invoices_id_seq;

CREATE SEQUENCE sklad.income_invoices_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.internal_movements_id_seq;

CREATE SEQUENCE sklad.internal_movements_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.inventories_id_seq;

CREATE SEQUENCE sklad.inventories_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.inventory_discrepancies_id_seq;

CREATE SEQUENCE sklad.inventory_discrepancies_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.inventory_discrepancies_id_seq1;

CREATE SEQUENCE sklad.inventory_discrepancies_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.inventory_id_seq;

CREATE SEQUENCE sklad.inventory_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.inventory_id_seq1;

CREATE SEQUENCE sklad.inventory_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.product_categories_id_seq;

CREATE SEQUENCE sklad.product_categories_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.product_categories_id_seq1;

CREATE SEQUENCE sklad.product_categories_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.products_id_new_seq;

CREATE SEQUENCE sklad.products_id_new_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 9223372036854775807
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.products_id_seq;

CREATE SEQUENCE sklad.products_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.products_id_seq1;

CREATE SEQUENCE sklad.products_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.racks_id_seq;

CREATE SEQUENCE sklad.racks_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.racks_id_seq1;

CREATE SEQUENCE sklad.racks_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.storage_locations_id_seq;

CREATE SEQUENCE sklad.storage_locations_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.storage_locations_id_seq1;

CREATE SEQUENCE sklad.storage_locations_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.storage_zones_id_seq;

CREATE SEQUENCE sklad.storage_zones_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.storage_zones_id_seq1;

CREATE SEQUENCE sklad.storage_zones_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.units_id_seq;

CREATE SEQUENCE sklad.units_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.units_id_seq1;

CREATE SEQUENCE sklad.units_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.users_id_seq;

CREATE SEQUENCE sklad.users_id_seq
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;
-- DROP SEQUENCE sklad.users_id_seq1;

CREATE SEQUENCE sklad.users_id_seq1
	INCREMENT BY 1
	MINVALUE 1
	MAXVALUE 2147483647
	START 1
	CACHE 1
	NO CYCLE;-- sklad.counter_ogents определение

-- Drop table

-- DROP TABLE sklad.counter_ogents;

CREATE TABLE sklad.counter_ogents (
	id int4 DEFAULT nextval('sklad.counterparties_id_seq'::regclass) NOT NULL,
	"name" varchar(200) NOT NULL,
	"type" varchar(10) NOT NULL,
	inn varchar(20) NULL,
	address text NULL,
	phone varchar(20) NULL,
	email varchar(100) NULL,
	contact_person varchar(100) NULL,
	is_active bool DEFAULT true NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	CONSTRAINT counterparties_pkey PRIMARY KEY (id),
	CONSTRAINT counterparties_type_check CHECK (((type)::text = ANY (ARRAY[('supplier'::character varying)::text, ('customer'::character varying)::text])))
);


-- sklad.storage_zones определение

-- Drop table

-- DROP TABLE sklad.storage_zones;

CREATE TABLE sklad.storage_zones (
	id serial4 NOT NULL,
	"name" varchar(100) NOT NULL,
	description text NULL,
	temperature_condition varchar(50) NULL,
	CONSTRAINT storage_zones_pkey PRIMARY KEY (id)
);


-- sklad.units определение

-- Drop table

-- DROP TABLE sklad.units;

CREATE TABLE sklad.units (
	id serial4 NOT NULL,
	"name" varchar(50) NOT NULL,
	short_name varchar(10) NOT NULL,
	CONSTRAINT units_pkey PRIMARY KEY (id),
	CONSTRAINT units_short_name_unique UNIQUE (short_name)
);


-- sklad.users определение

-- Drop table

-- DROP TABLE sklad.users;

CREATE TABLE sklad.users (
	id serial4 NOT NULL,
	username varchar(50) NOT NULL,
	"password" varchar(255) NOT NULL,
	full_name varchar(100) NOT NULL,
	email varchar(100) NULL,
	"role" varchar(20) DEFAULT 'operator'::character varying NULL,
	is_active bool DEFAULT true NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	last_login timestamp NULL,
	CONSTRAINT users_pkey PRIMARY KEY (id),
	CONSTRAINT users_role_check CHECK (((role)::text = ANY (ARRAY[('admin'::character varying)::text, ('manager'::character varying)::text, ('operator'::character varying)::text]))),
	CONSTRAINT users_username_key UNIQUE (username)
);


-- sklad.ostatki определение

-- Drop table

-- DROP TABLE sklad.ostatki;

CREATE TABLE sklad.ostatki (
	id int4 DEFAULT nextval('sklad.inventories_id_seq'::regclass) NOT NULL,
	inventory_number varchar(50) NOT NULL,
	start_date date NOT NULL,
	end_date date NULL,
	status varchar(20) DEFAULT 'planned'::character varying NULL,
	description text NULL,
	created_by int4 NOT NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	CONSTRAINT inventories_inventory_number_key UNIQUE (inventory_number),
	CONSTRAINT inventories_pkey PRIMARY KEY (id),
	CONSTRAINT inventories_status_check CHECK (((status)::text = ANY (ARRAY[('planned'::character varying)::text, ('in_progress'::character varying)::text, ('completed'::character varying)::text, ('cancelled'::character varying)::text]))),
	CONSTRAINT inventories_created_by_fkey FOREIGN KEY (created_by) REFERENCES sklad.users(id)
);


-- sklad.postuplenia определение

-- Drop table

-- DROP TABLE sklad.postuplenia;

CREATE TABLE sklad.postuplenia (
	id int4 DEFAULT nextval('sklad.income_invoices_id_seq'::regclass) NOT NULL,
	invoice_number varchar(50) NOT NULL,
	supplier_id int4 NOT NULL,
	invoice_date date NOT NULL,
	arrival_date date NULL,
	total_amount numeric(40, 2) DEFAULT 0 NULL,
	status text DEFAULT 'draft'::character varying NULL,
	created_by int4 NOT NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	updated_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	CONSTRAINT income_invoices_invoice_number_key UNIQUE (invoice_number),
	CONSTRAINT income_invoices_pkey PRIMARY KEY (id),
	CONSTRAINT income_invoices_status_check CHECK ((status = ANY (ARRAY[('draft'::character varying)::text, ('received'::character varying)::text, ('cancelled'::character varying)::text]))),
	CONSTRAINT income_invoices_created_by_fkey FOREIGN KEY (created_by) REFERENCES sklad.users(id),
	CONSTRAINT income_invoices_supplier_id_fkey FOREIGN KEY (supplier_id) REFERENCES sklad.counter_ogents(id)
);
CREATE INDEX idx_income_invoices_date ON sklad.postuplenia USING btree (invoice_date);

-- Table Triggers

create trigger update_income_invoices_updated_at before
update
    on
    sklad.postuplenia for each row execute function sklad.update_updated_at_column();


-- sklad.product_categories определение

-- Drop table

-- DROP TABLE sklad.product_categories;

CREATE TABLE sklad.product_categories (
	id serial4 NOT NULL,
	"name" varchar(100) NOT NULL,
	parent_id int4 NULL,
	description text NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	CONSTRAINT product_categories_pkey PRIMARY KEY (id),
	CONSTRAINT product_categories_parent_id_fkey FOREIGN KEY (parent_id) REFERENCES sklad.product_categories(id)
);


-- sklad.products определение

-- Drop table

-- DROP TABLE sklad.products;

CREATE TABLE sklad.products (
	id serial4 NOT NULL,
	sku varchar(50) NOT NULL,
	"name" varchar(200) NOT NULL,
	description text NULL,
	category_id int4 NULL,
	unit_id int4 NOT NULL,
	weight numeric(10, 3) NULL,
	volume numeric(10, 3) NULL,
	min_stock_level int4 DEFAULT 0 NULL,
	max_stock_level int4 NULL,
	purchase_price numeric(15, 2) NULL,
	selling_price numeric(15, 2) NULL,
	barcode varchar(100) NULL,
	is_active bool DEFAULT true NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	updated_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	CONSTRAINT products_pkey PRIMARY KEY (id),
	CONSTRAINT products_sku_key UNIQUE (sku),
	CONSTRAINT products_category_id_fkey FOREIGN KEY (category_id) REFERENCES sklad.product_categories(id),
	CONSTRAINT products_unit_id_fkey FOREIGN KEY (unit_id) REFERENCES sklad.units(id)
);
CREATE INDEX idx_products_category ON sklad.products USING btree (category_id);
CREATE INDEX idx_products_sku ON sklad.products USING btree (sku);

-- Table Triggers

create trigger update_products_updated_at before
update
    on
    sklad.products for each row execute function sklad.update_updated_at_column();


-- sklad.racks определение

-- Drop table

-- DROP TABLE sklad.racks;

CREATE TABLE sklad.racks (
	id serial4 NOT NULL,
	zone_id int4 NOT NULL,
	"name" varchar(50) NOT NULL,
	description text NULL,
	CONSTRAINT racks_pkey PRIMARY KEY (id),
	CONSTRAINT racks_zone_id_fkey FOREIGN KEY (zone_id) REFERENCES sklad.storage_zones(id)
);


-- sklad.schet_faktura определение

-- Drop table

-- DROP TABLE sklad.schet_faktura;

CREATE TABLE sklad.schet_faktura (
	id int4 DEFAULT nextval('sklad.expense_invoices_id_seq'::regclass) NOT NULL,
	invoice_number varchar(50) NOT NULL,
	customer_id int4 NOT NULL,
	invoice_date date NOT NULL,
	shipment_date date NULL,
	total_amount numeric(15, 2) DEFAULT 0 NULL,
	status varchar(20) DEFAULT 'draft'::character varying NULL,
	created_by int4 NOT NULL,
	created_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	updated_at timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	CONSTRAINT expense_invoices_invoice_number_key UNIQUE (invoice_number),
	CONSTRAINT expense_invoices_pkey PRIMARY KEY (id),
	CONSTRAINT expense_invoices_status_check CHECK (((status)::text = ANY (ARRAY[('draft'::character varying)::text, ('shipped'::character varying)::text, ('cancelled'::character varying)::text]))),
	CONSTRAINT expense_invoices_created_by_fkey FOREIGN KEY (created_by) REFERENCES sklad.users(id),
	CONSTRAINT expense_invoices_customer_id_fkey FOREIGN KEY (customer_id) REFERENCES sklad.counter_ogents(id)
);
CREATE INDEX idx_expense_invoices_date ON sklad.schet_faktura USING btree (invoice_date);

-- Table Triggers

create trigger update_expense_invoices_updated_at before
update
    on
    sklad.schet_faktura for each row execute function sklad.update_updated_at_column();


-- sklad.schet_faktura_soderzanie определение

-- Drop table

-- DROP TABLE sklad.schet_faktura_soderzanie;

CREATE TABLE sklad.schet_faktura_soderzanie (
	id int4 DEFAULT nextval('sklad.expense_invoice_items_id_seq'::regclass) NOT NULL,
	invoice_id int4 NOT NULL,
	product_id int4 NOT NULL,
	quantity int4 NOT NULL,
	unit_price numeric(15, 2) NOT NULL,
	total_price numeric(15, 2) GENERATED ALWAYS AS ((quantity::numeric * unit_price)) STORED NULL,
	picked_quantity int4 DEFAULT 0 NULL,
	picked_at timestamp NULL,
	picked_by int4 NULL,
	CONSTRAINT expense_invoice_items_invoice_id_product_id_key UNIQUE (invoice_id, product_id),
	CONSTRAINT expense_invoice_items_pkey PRIMARY KEY (id),
	CONSTRAINT expense_invoice_items_quantity_check CHECK ((quantity > 0)),
	CONSTRAINT expense_invoice_items_invoice_id_fkey FOREIGN KEY (invoice_id) REFERENCES sklad.schet_faktura(id) ON DELETE CASCADE,
	CONSTRAINT expense_invoice_items_picked_by_fkey FOREIGN KEY (picked_by) REFERENCES sklad.users(id),
	CONSTRAINT expense_invoice_items_product_id_fkey FOREIGN KEY (product_id) REFERENCES sklad.products(id)
);


-- sklad.storage_locations определение

-- Drop table

-- DROP TABLE sklad.storage_locations;

CREATE TABLE sklad.storage_locations (
	id serial4 NOT NULL,
	rack_id int4 NOT NULL,
	location_code varchar(50) NOT NULL,
	"level" int4 NULL,
	"position" int4 NULL,
	width numeric(8, 2) NULL,
	height numeric(8, 2) NULL,
	"depth" numeric(8, 2) NULL,
	max_weight numeric(10, 2) NULL,
	is_occupied bool DEFAULT false NULL,
	CONSTRAINT storage_locations_location_code_key UNIQUE (location_code),
	CONSTRAINT storage_locations_pkey PRIMARY KEY (id),
	CONSTRAINT storage_locations_rack_id_fkey FOREIGN KEY (rack_id) REFERENCES sklad.racks(id)
);
CREATE INDEX idx_storage_locations_code ON sklad.storage_locations USING btree (location_code);


-- sklad.vnutrinie_movements определение

-- Drop table

-- DROP TABLE sklad.vnutrinie_movements;

CREATE TABLE sklad.vnutrinie_movements (
	id int4 DEFAULT nextval('sklad.internal_movements_id_seq'::regclass) NOT NULL,
	movement_number varchar(50) NOT NULL,
	product_id int4 NOT NULL,
	from_location_id int4 NOT NULL,
	to_location_id int4 NOT NULL,
	quantity int4 NOT NULL,
	movement_date timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	reason varchar(200) NULL,
	moved_by int4 NOT NULL,
	completed bool DEFAULT false NULL,
	CONSTRAINT internal_movements_movement_number_key UNIQUE (movement_number),
	CONSTRAINT internal_movements_pkey PRIMARY KEY (id),
	CONSTRAINT internal_movements_quantity_check CHECK ((quantity > 0)),
	CONSTRAINT internal_movements_from_location_id_fkey FOREIGN KEY (from_location_id) REFERENCES sklad.storage_locations(id),
	CONSTRAINT internal_movements_moved_by_fkey FOREIGN KEY (moved_by) REFERENCES sklad.users(id),
	CONSTRAINT internal_movements_product_id_fkey FOREIGN KEY (product_id) REFERENCES sklad.products(id),
	CONSTRAINT internal_movements_to_location_id_fkey FOREIGN KEY (to_location_id) REFERENCES sklad.storage_locations(id)
);


-- sklad.inventory определение

-- Drop table

-- DROP TABLE sklad.inventory;

CREATE TABLE sklad.inventory (
	id serial4 NOT NULL,
	product_id int4 NOT NULL,
	location_id int4 NOT NULL,
	kolichestvo int4 DEFAULT 0 NOT NULL,
	batch_number varchar(100) NULL,
	production_date date NULL,
	spisanie_date date NULL,
	serial_number varchar(100) NULL,
	last_updated timestamp DEFAULT CURRENT_TIMESTAMP NULL,
	CONSTRAINT inventory_pkey PRIMARY KEY (id),
	CONSTRAINT inventory_product_id_location_id_batch_number_serial_number_key UNIQUE (product_id, location_id, batch_number, serial_number),
	CONSTRAINT inventory_quantity_check CHECK ((kolichestvo >= 0)),
	CONSTRAINT inventory_location_id_fkey FOREIGN KEY (location_id) REFERENCES sklad.storage_locations(id),
	CONSTRAINT inventory_product_id_fkey FOREIGN KEY (product_id) REFERENCES sklad.products(id)
);
CREATE INDEX idx_inventory_expiration ON sklad.inventory USING btree (spisanie_date);
CREATE INDEX idx_inventory_location ON sklad.inventory USING btree (location_id);
CREATE INDEX idx_inventory_product ON sklad.inventory USING btree (product_id);


-- sklad.inventory_discrepancies определение

-- Drop table

-- DROP TABLE sklad.inventory_discrepancies;

CREATE TABLE sklad.inventory_discrepancies (
	id serial4 NOT NULL,
	inventory_id int4 NOT NULL,
	product_id int4 NOT NULL,
	location_id int4 NOT NULL,
	expected_quantity int4 NOT NULL,
	actual_quantity int4 DEFAULT 0 NOT NULL,
	discrepancy int4 GENERATED ALWAYS AS (actual_quantity - expected_quantity) STORED NULL,
	discrepancy_reason varchar(200) NULL,
	resolved bool DEFAULT false NULL,
	resolved_at timestamp NULL,
	resolved_by int4 NULL,
	CONSTRAINT inventory_discrepancies_inventory_id_product_id_location_id_key UNIQUE (inventory_id, product_id, location_id),
	CONSTRAINT inventory_discrepancies_pkey PRIMARY KEY (id),
	CONSTRAINT inventory_discrepancies_inventory_id_fkey FOREIGN KEY (inventory_id) REFERENCES sklad.ostatki(id) ON DELETE CASCADE,
	CONSTRAINT inventory_discrepancies_location_id_fkey FOREIGN KEY (location_id) REFERENCES sklad.storage_locations(id),
	CONSTRAINT inventory_discrepancies_product_id_fkey FOREIGN KEY (product_id) REFERENCES sklad.products(id),
	CONSTRAINT inventory_discrepancies_resolved_by_fkey FOREIGN KEY (resolved_by) REFERENCES sklad.users(id)
);


-- sklad.postuplenia_items определение

-- Drop table

-- DROP TABLE sklad.postuplenia_items;

CREATE TABLE sklad.postuplenia_items (
	id int4 DEFAULT nextval('sklad.income_invoice_items_id_seq'::regclass) NOT NULL,
	invoice_id int4 NOT NULL,
	product_id int4 NOT NULL,
	location_id int4 NOT NULL,
	quantity int4 NOT NULL,
	unit_price numeric(15, 2) NOT NULL,
	total_price numeric(15, 2) GENERATED ALWAYS AS ((quantity::numeric * unit_price)) STORED NULL,
	batch_number varchar(100) NULL,
	production_date date NULL,
	expiration_date date NULL,
	serial_number varchar(100) NULL,
	CONSTRAINT income_invoice_items_invoice_id_product_id_location_id_key UNIQUE (invoice_id, product_id, location_id),
	CONSTRAINT income_invoice_items_pkey PRIMARY KEY (id),
	CONSTRAINT income_invoice_items_quantity_check CHECK ((quantity > 0)),
	CONSTRAINT income_invoice_items_invoice_id_fkey FOREIGN KEY (invoice_id) REFERENCES sklad.postuplenia(id) ON DELETE CASCADE,
	CONSTRAINT income_invoice_items_location_id_fkey FOREIGN KEY (location_id) REFERENCES sklad.storage_locations(id),
	CONSTRAINT income_invoice_items_product_id_fkey FOREIGN KEY (product_id) REFERENCES sklad.products(id)
);



-- DROP FUNCTION sklad.update_updated_at_column();

CREATE OR REPLACE FUNCTION sklad.update_updated_at_column()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$function$
;
