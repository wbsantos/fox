CREATE OR REPLACE PROCEDURE  fox_document_delete_V1(_id uuid)
LANGUAGE SQL
BEGIN ATOMIC
	DELETE S FROM Stamp JOIN DocumentPermission D ON D.stampId = S.Id WHERE D.documentId = _id
	DELETE FROM DocumentPermission WHERE documentId = _id
	DELETE FROM DocumentMetadata WHERE documentId = _id
	DELETE S FROM Stamp JOIN Document D ON D.stampId = S.Id WHERE D.id = _id
	DELETE FROM Document WHERE id = _id
END