CREATE OR REPLACE PROCEDURE  fox_document_delpermission_v1 (
			_documentId uuid,
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM DocumentPermission
	WHERE
		documentId = _documentId
		AND holderId = _holderId
		AND permission = _permission;
END
$$;