CREATE OR REPLACE FUNCTION fox_document_create_v1 (
			_stamp_id int,
			_file_binary bytea,
			_fileName varchar(255),
			_sizeBytes int
)
RETURNS UUID
LANGUAGE plpgsql AS
$$
DECLARE _document_id uuid;
BEGIN
	INSERT INTO Document
	(
		file_binary,
		stamp_id,
		name,
		file_size_bytes
    )
	VALUES
	(
		_file_binary,
		_stamp_id,
		_fileName,
		_sizeBytes
    ) RETURNING id INTO _document_id;
   
	RETURN _document_id;
END
$$