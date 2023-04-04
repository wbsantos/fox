CREATE OR REPLACE FUNCTION  fox_document_create_v1 (
			_stampId int,
			_fileBinary bytea,
			_fileName varchar(255),
			_sizeBytes int
)
RETURNS UUID
LANGUAGE plpgsql AS
$$
DECLARE _documentId uuid;
BEGIN
	INSERT INTO Document
	(
		fileBinary,
		stampId,
		name,
		fileSizeBytes
    )
	VALUES
	(
		_fileBinary,
		_stampId,
		_fileName,
		_sizeBytes
    ) RETURNING id INTO _documentId;
   
	RETURN _documentId;
END
$$