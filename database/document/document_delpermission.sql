CREATE OR REPLACE PROCEDURE fox_document_delpermission_v1 (
			_document_id uuid,
			_holder_id uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM document_permission
	WHERE
		document_id = _document_id
		AND holder_id = _holder_id
		AND permission = _permission;
END
$$;