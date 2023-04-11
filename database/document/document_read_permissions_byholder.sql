CREATE OR REPLACE FUNCTION fox_document_read_permissions_byholder_v1(_document_id uuid, _holder_id uuid)
RETURNS TABLE (
	permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query 
		SELECT
			P1.permission
		FROM document_permission P1
		WHERE
			P1.document_id = _document_id
			AND P1.holder_id  = _holder_id
		UNION
		SELECT
			P2.permission
		FROM user_group U
		INNER JOIN document_permission P2 ON P2.holder_id  = U.group_id 
		WHERE
			U.user_id = _holder_id;
END
$$;