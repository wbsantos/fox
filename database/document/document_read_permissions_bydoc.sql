CREATE OR REPLACE FUNCTION fox_document_read_permissions_bydoc_v1(_document_id uuid)
RETURNS TABLE (
	holder_id uuid,
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
			P1.holder_id,
			U.login,
			U.name,
			P1.permission,
			'User'::varchar(10) AS holderType
		FROM document_permission P1
		INNER JOIN user_account U ON U.id = P1.holder_id
		WHERE
			P1.document_id = _document_id
		UNION
		SELECT
			P2.holder_id,
			''::varchar(63),
			G.name,
			P2.permission,
			'Group'::varchar(10) AS holderType
		FROM document_permission P2
		INNER JOIN group_account G ON G.id = P2.holder_id
		WHERE
			P2.document_id = _document_id;

END
$$;