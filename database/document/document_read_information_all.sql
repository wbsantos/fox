CREATE OR REPLACE FUNCTION fox_document_read_information_all_v1(_userId uuid)
RETURNS TABLE (
	id uuid,
	name varchar(255),
	fileSizeBytes int
)
LANGUAGE plpgsql AS
$$
BEGIN
	RETURN query
		SELECT
			D1.id,
			D1.name,
			D1.filesizebytes 
		FROM Document D1
		INNER JOIN DocumentPermission P1 ON
			P1.documentId = D1.id
			AND P1.holderId = _userId
		UNION
		SELECT
			D2.id,
			D2.name,
			D2.filesizebytes 
		FROM Document D2
		INNER JOIN UserGroup UG ON
			UG.userId = _userId
		INNER JOIN DocumentPermission P2 ON
			P2.documentId = D2.id
			AND P2.holderId = UG.groupId;
END
$$;