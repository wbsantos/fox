--create database fox
/* 
// To recreate all tables, they can be dropped using the commands bellow
drop table DocumentPermission;
drop table DocumentMetadata;
drop table Document;
drop table SystemPermission;
drop table UserGroup;
drop table Stamp;
drop table UserAccount;
drop table GroupAccount;
drop table Holder;
*/

create table if not exists Holder --it's either an user or a group id
(
	id uuid default gen_random_uuid() not null,

	CONSTRAINT PK_HOLDER PRIMARY KEY(id)
);

create table if not exists GroupAccount
(
	id uuid default gen_random_uuid() not null,
	name varchar(255) not null,

	CONSTRAINT PK_GROUPACCOUNT PRIMARY KEY(id),
	CONSTRAINT FK_GROUPACCOUNT_HOLDER FOREIGN KEY (id) REFERENCES Holder(id)
);

create table if not exists UserAccount
(
	id uuid default gen_random_uuid() not null,
	email varchar(255) not null,
	login varchar(63) not null,
	password bytea not null,
	salt bytea not null,
	hashMethod int not null,
	name varchar(255) not null,

	CONSTRAINT PK_USERACCOUNT PRIMARY KEY(id),
	CONSTRAINT FK_USERACCOUNT_HOLDER FOREIGN KEY (id) REFERENCES Holder(id)
);

create table if not exists Stamp
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	userId uuid,
	systemVersion varchar(255),
	createdAt timestamptz,

	CONSTRAINT PK_STAMP PRIMARY KEY(id),
	CONSTRAINT FK_STAMP_USERACCOUNT FOREIGN KEY (userId) REFERENCES UserAccount(id)
);

create table if not exists UserGroup
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	userId uuid not null,
	groupId uuid not null,
	stampId int not null,

	CONSTRAINT PK_USERGROUP PRIMARY KEY(id),
	CONSTRAINT FK_USERGROUP_USERACCOUNT FOREIGN KEY (userId) REFERENCES UserAccount(id),
	CONSTRAINT FK_USERGROUP_GROUPACCOUNT FOREIGN KEY (groupId) REFERENCES GroupAccount(id)
);

create table if not exists SystemPermission
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	stampId int not null,
	holderId uuid not null,
	permission varchar(255) not null,

	CONSTRAINT PK_SYSTEMPERMISSION PRIMARY KEY(id),
	CONSTRAINT FK_SYSTEMPERMISSION_HOLDER FOREIGN KEY (holderId) REFERENCES Holder(id),
	CONSTRAINT FK_SYSTEMPERMISSION_STAMP FOREIGN KEY (stampId) REFERENCES Stamp(id)	
);

create table if not exists Document
(
	id uuid default gen_random_uuid() not null,
	stampId int not null,
	name varchar(255),
	fileBinary bytea not null,
	fileSizeBytes int,

	CONSTRAINT PK_DOCUMENT PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENT_STAMP FOREIGN KEY (stampId) REFERENCES Stamp(id)	
);

create table if not exists DocumentMetadata
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	documentId uuid not null,
	key varchar(255) not null,
	value varchar(1023),

	CONSTRAINT PK_DOCUMENTMETADATA PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENTMETADATA_DOCUMENT FOREIGN KEY (documentId) REFERENCES Document(id)
);

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
);

--CREATING INDEXES
CREATE UNIQUE INDEX UN_USERACCOUNT_01 ON UserAccount(login);
CREATE UNIQUE INDEX UN_GROUPACCOUNT_01 ON GroupAccount(name);
CREATE UNIQUE INDEX UN_USERGROUP_01 ON UserGroup(groupId, userId);
CREATE INDEX IN_STAMP_01 ON Stamp(userId);
CREATE INDEX IN_STAMP_02 ON Stamp(createdAt);
CREATE INDEX IN_USERGROUP_01 ON UserGroup(userId);
CREATE INDEX IN_USERGROUP_02 ON UserGroup(groupId);
CREATE INDEX IN_SYSTEMPERMISSION_01 ON SystemPermission(holderId);
CREATE INDEX IN_DOCUMENT_01 ON Document(name);
CREATE INDEX IN_DOCUMENTMETADATA_01 ON DocumentMetadata(key);
CREATE INDEX IN_DOCUMENTPERMISSION_01 ON DocumentPermission(holderId);
CREATE INDEX IN_DOCUMENTPERMISSION_02 ON DocumentPermission(documentId);
