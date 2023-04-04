CREATE OR REPLACE PROCEDURE  fox_document_delpermission_v1 (
			_documentId uuid,
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM Stamp S
	USING DocumentPermission P 
	WHERE
		P.stampId = S.Id
		AND P.documentId = _documentId
		AND P.holderId = _holderId
		AND P.permission = _permission;

	DELETE FROM DocumentPermission
	WHERE
		documentId = _documentId
		AND holderId = _holderId
		AND permission = _permission;
END
$$;