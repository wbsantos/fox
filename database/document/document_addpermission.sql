CREATE OR REPLACE PROCEDURE  fox_document_addpermission_V1 (
			_stampId int,
			_documentId uuid,
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE SQL
BEGIN ATOMIC
	INSERT INTO DocumentPermission (stampId, documentId, holderId, permission)
	VALUES (_stampId, _documentId, _holderId, _permission)
END