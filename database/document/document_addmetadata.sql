CREATE OR REPLACE PROCEDURE  fox_document_addmetadata_V1 (
			_documentId uuid,
			_keys varchar(255)[],
			_values varchar(1023)[]
)
LANGUAGE SQL
BEGIN ATOMIC
	INSERT INTO DocumentMetadata (documentId, key, value)
	SELECT _documentId, _key, _value
	FROM UNNEST(_keys) WITH ORDINALITY AS keys (_key, _i)
	JOIN UNNEST(_values) WITH ORDINALITY AS values (_value, _j) ON keys._i = values._j
END