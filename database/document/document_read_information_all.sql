CREATE OR REPLACE FUNCTION fox_document_read_information_all_v1(_user_id uuid)
RETURNS TABLE (
	id uuid,
	name varchar(255),
	file_size_bytes int
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query
		SELECT
			D1.id,
			D1.name,
			D1.file_size_bytes 
		FROM Document D1
		INNER JOIN document_permission P1 ON
			P1.document_id = D1.id
			AND P1.holder_id = _user_id
		UNION
		SELECT
			D2.id,
			D2.name,
			D2.file_size_bytes 
		FROM Document D2
		INNER JOIN user_group UG ON
			UG.user_id = _user_id
		INNER JOIN document_permission P2 ON
			P2.document_id = D2.id
			AND P2.holder_id = UG.group_id;
END
$$;