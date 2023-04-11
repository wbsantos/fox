CREATE OR REPLACE PROCEDURE fox_document_delete_v1(_id uuid)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM document_permission WHERE document_id = _id;
	DELETE FROM document_metadata WHERE document_id = _id;
	DELETE FROM Document WHERE id = _id;
END
$$;