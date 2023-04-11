CREATE OR REPLACE PROCEDURE fox_document_addmetadata_v1 (
			_document_id uuid,
			_keys varchar(255)[],
			_values varchar(1023)[]
)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM document_metadata
	WHERE 
		document_id = _document_id
		AND key = ANY(_keys);
	
	INSERT INTO document_metadata (document_id, key, value)
	SELECT _document_id, _key, _value
	FROM UNNEST(_keys) WITH ORDINALITY AS keys (_key, _i)
	JOIN UNNEST(_values) WITH ORDINALITY AS values (_value, _j) ON keys._i = values._j;
END
$$;