# Fox Application
WebApp for uploading and managing access to files and metadata

This application allows users to upload and download documents (with metadata).
It is also possible to manage other users' access to the documents. 
An user can belong to one or more groups, access to documents can be given to users and/or groups.

An user or group can also have access to managing other users or groups.

# Database
Under the folder "/database" you can find:
- container-setup.txt: Contains the command line for creating a container with a postgres server (using PODMAN). It is not mandatory to use this if you have your on postgres server.
- db-setup: A script for creating all tables and indexes. You should create your database beforehand using "CREATE DATABASE fox"
- Other folders (such as document, group, stamp, ...): The subfolders contain all procedures used by the application. You don't have to worry about it during setup, the application will automatically create all the procedures it needs

I do recommend using different databases for development and the unit tests. On the API.Fox project there are three differents appsettings.json
- appsettings.json: for production
- appsettings.Development.json: for development
- appsettings.Test.json: for unit testing

So it is possible to configure different connection string for each environment.

# API
Under the folder "/api" you can find the solution "API.Fox.sln".
The API solution contains 32 endpoints for managing all aspects of the application.

Included in this solution is an "End to End test project" called "API.Test.Fox", it tests all endpoints of the API project.

The API was built using .NET 6 minimal API, so there is no boilerplate code.

# Web
Under the folder "/api" you can find the solution "Web.Fox.sln".

The Web project uses the same DLL's as the API project. The pages were built with Razor pages and uses dependency injection (as the API does) for access to the database and models.

# Access level
All actions can individually be given permission to any user or group. Because of that there are no hard coded "user roles".
To mimic roles functionality do the following:
1. Create any user role you think is relevant as a Group. E.g.: Manager and User.
2. Set the Manager group to have access to all the system operations
3. Set the User group to have access only for upload and download of  documents

Access to documents is given and taken independently of the system permissions.

As an exception, the application uses an "admin" claim to bypass any other verification. On startup the application will automatically create an admin user using the applicationsettings.json definition.
By default the admin user (on development environment) will be:
- User name for login: admin
- Password: 123456 

# List of API's
- /security/token|POST: create JWT token (mandatory for all actions)
- /selfmanagement/user|GET: get information about the user logged in
- /selfmanagement/user/group|GET: get the groups the logged user belongs to
- /selfmanagement/user|PUT: update data of the logged user
- /selfmanagement/user/password|PUT: update password of the logged user
- /management/user|POST: create a new user
- /management/user|DELETE: delete an user
- /management/user|GET: get information about an user
- /management/user/all|GET: get all users
- /management/user/group|GET: get the groups an user belongs to
- /management/user|PUT: update information about an user
- /management/user/password|PUT: update an user's password
- /management/systempermission|POST: add permission to perform some action in the system to an user or group
- /management/systempermission|DELETE: remove permission to perform some action in the system from an user or group
- /management/systempermission|GET: read the permissions an user or group currently has
- /management/group|POST: create a new group
- /management/group|DELETE: delete a group
- /management/group|GET: get information about a group
- /management/group|PUT: update a group information
- /management/group/all|GET: get all groups
- /management/group/user|POST: add one or more users to a group
- /management/group/user|PUT: delete one or more users from a group
- /management/group/user|GET: get users that are in a specif group
- /document|POST: upload a new document (file binary, name and metadata) 
- /document|PUT: update document information (name and metadata)
- /document|DELETE: delete a document
- /document|GET: get document information (no binary)
- /document/all|GET: get all documents information (no binary)
- /document/download|GET: download a document binary
- /document/permission|POST: add access for an user or group to a document
- /document/permission|DELETE: remove access for an user or group to a document
- /document/permission|GET: get users and group that currently have acces to a document