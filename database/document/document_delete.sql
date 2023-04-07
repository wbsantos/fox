CREATE OR REPLACE PROCEDURE  fox_document_delete_v1(_id uuid)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM DocumentPermission WHERE documentId = _id;
	DELETE FROM DocumentMetadata WHERE documentId = _id;
	DELETE FROM Document WHERE id = _id;
END
$$;