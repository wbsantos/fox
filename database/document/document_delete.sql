CREATE OR REPLACE PROCEDURE  fox_document_delete_v1(_id uuid)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM Stamp S USING DocumentPermission D WHERE D.stampId = S.Id AND D.documentId = _id;
	DELETE FROM DocumentPermission WHERE documentId = _id;
	DELETE FROM DocumentMetadata WHERE documentId = _id;
	DELETE FROM Stamp S USING Document D WHERE D.stampId = S.Id AND D.id = _id;
	DELETE FROM Document WHERE id = _id;
END
$$;