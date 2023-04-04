CREATE OR REPLACE FUNCTION  fox_document_read_permissions_bydoc_v1(_documentId uuid)
RETURNS TABLE (
	holderId uuid,
	holderName varchar(255),
	permission varchar(255),
	holderType varchar(10)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			P1.holderid,
			U.name,
			P1.permission,
			'User' AS holderType
		FROM DocumentPermission P1
		INNER JOIN UserAccount U ON U.id = P1.holderId
		WHERE
			P1.documentId = _documentId
		UNION
		SELECT
			P1.holderid,
			G.name,
			P1.permission,
			'Group' AS holderType
		FROM DocumentPermission P1
		INNER JOIN GroupAccount U ON G.id = P1.holderId
		WHERE
			P1.documentId = _documentId;

END
$$;