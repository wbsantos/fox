CREATE OR REPLACE PROCEDURE  fox_document_delete_V1(_id uuid)
LANGUAGE SQL
BEGIN ATOMIC
	DELETE FROM DocumentPermission WHERE documentId = _id
	DELETE FROM DocumentMetadata WHERE documentId = _id
	DELETE S FROM Stamp JOIN Document ON Document.stampId = Stamp.Id WHERE Document.id = _id
	DELETE FROM Document WHERE id = _id
END