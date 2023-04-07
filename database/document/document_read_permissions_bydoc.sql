CREATE OR REPLACE FUNCTION  fox_document_read_permissions_bydoc_v1(_documentId uuid)
RETURNS TABLE (
	holderId uuid,
	holderLogin varchar(63),
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
			U.login,
			U.name,
			P1.permission,
			'User'::varchar(10) AS holderType
		FROM DocumentPermission P1
		INNER JOIN UserAccount U ON U.id = P1.holderId
		WHERE
			P1.documentId = _documentId
		UNION
		SELECT
			P2.holderid,
			''::varchar(63),
			G.name,
			P2.permission,
			'Group'::varchar(10) AS holderType
		FROM DocumentPermission P2
		INNER JOIN GroupAccount G ON G.id = P2.holderId
		WHERE
			P2.documentId = _documentId;

END
$$;