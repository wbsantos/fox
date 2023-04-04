CREATE OR REPLACE FUNCTION  fox_document_read_permissions_byholder_v1(_documentId uuid, _holderId uuid)
RETURNS TABLE (
	permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			P1.permission
		FROM DocumentPermission P1
		WHERE
			P1.documentId = _documentId
			AND P1.holderid  = _holderId
		UNION
		SELECT
			P2.permission
		FROM UserGroup U
		INNER JOIN DocumentPermission P2 ON P2.holderid  = U.groupid 
		WHERE
			U.userId = _holderId;
END
$$;