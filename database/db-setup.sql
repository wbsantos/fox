--create database fox

create table if not exists Holder --it's either an user or a group id
(
	id uuid default gen_random_uuid() not null,

	CONSTRAINT PK_HOLDER PRIMARY KEY(id)
)

create table if not exists Group
(
	id uuid default gen_random_uuid() not null,
	name varchar(255) not null,

	CONSTRAINT PK_GROUP PRIMARY KEY(id),
	CONSTRAINT FK_GROUP_HOLDER FOREIGN KEY (id) REFERENCES Holder(id),
)

create table if not exists User
(
	id uuid default gen_random_uuid() not null,
	email varchar(255) not null,
	login varchar(63) not null,
	password bytea not null,
	salt bytea not null,
	hashMethod smallint not null,
	name varchar(255) not null,

	CONSTRAINT PK_USER PRIMARY KEY(id),
	CONSTRAINT FK_USER_HOLDER FOREIGN KEY (id) REFERENCES Holder(id),
)

create table if not exists UserGroup
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	userId uuid not null,
	groupId uuid not null,
	stampId int not null,

	CONSTRAINT PK_USERGROUP PRIMARY KEY(id),
	CONSTRAINT FK_USERGROUP_USER FOREIGN KEY (userId) REFERENCES User(id),
	CONSTRAINT FK_USERGROUP_GROUP FOREIGN KEY (groupId) REFERENCES Group(id)
)

create table if not exists SystemPermission
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	holderId uuid not null,
	permission varchar(255) not null,

	CONSTRAINT PK_SYSTEMPERMISSION PRIMARY KEY(id),
	CONSTRAINT FK_USERGROUP_HOLDER FOREIGN KEY (holderId) REFERENCES Holder(id)
)

create table if not exists Stamp
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	userId uuid,
	systemVersion varchar(255),
	createdAt timestamptz,

	CONSTRAINT PK_STAMP PRIMARY KEY(id),
	CONSTRAINT FK_STAMP_USER FOREIGN KEY (userId) REFERENCES User(id)
)

create table if not exists Document
(
	id uuid default gen_random_uuid() not null,
	stampId int not null,
	name varchar(255),
	fileBinary bytea not null,
	fileSizeBytes int,

	CONSTRAINT PK_DOCUMENT PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENT_STAMP FOREIGN KEY (stampId) REFERENCES Stamp(id)	
)

create table if not exists DocumentMetadata
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	documentId uuid not null,
	key varchar(255) not null,
	value varchar(1023),

	CONSTRAINT PK_DOCUMENTMETADATA PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENTMETADATA_DOCUMENT FOREIGN KEY (documentId) REFERENCES Document(id)
)

create table if not exists DocumentPermission
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	stampId int not null,
	holderId uuid not null, 
	documentId uuid not null,
	permission varchar(255) not null,

	CONSTRAINT PK_DOCUMENTPERMISSION PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENTPERMISSION_HOLDER FOREIGN KEY (holderId) REFERENCES Holder(id),
	CONSTRAINT FK_DOCUMENTPERMISSION_DOCUMENT FOREIGN KEY (documentId) REFERENCES Document(id),
	CONSTRAINT FK_DOCUMENTPERMISSION_STAMP FOREIGN KEY (stampId) REFERENCES Stamp(id)
)

/*

TODO: Define indexes
TODO: Write stored procedures regarding System permissions

Implement a simple document storage api
● User must be able to log in
● Users can upload and download documents with metadata (posted date, name, description, and category)
● Users can create groups, and manage users

● Implement groups: a user can belong to one or more groups
● Document access can be granted to groups or directly to users
● Implement at least three roles:
	○ regular user can download documents
	○ manager user can upload and download documents
	○ admin can crud users, crud groups, upload documents and download documents

● Make sure to create database tables and stored procedures. You can create .NET object
to db type mapper

*/