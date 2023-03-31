CREATE OR REPLACE PROCEDURE  fox_document_delpermission_V1 (
			_documentId uuid,
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE SQL
BEGIN ATOMIC
	DELETE S FROM Stamp 
	JOIN DocumentPermission P ON P.stampId = S.Id 
	WHERE
		P.documentId = _documentId
		AND P.holderId = _holderId
		AND P.permission = _permission

	DELETE FROM DocumentPermission
	WHERE
		documentId = _documentId
		AND holderId = _holderId
		AND permission = _permission
END