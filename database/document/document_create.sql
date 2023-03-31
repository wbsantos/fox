CREATE OR REPLACE PROCEDURE  fox_document_create_V1 (
			_stampId int,
			_fileBinary bytea,
			_fileName varchar(255),
			_sizeBytes int
)
LANGUAGE SQL
BEGIN ATOMIC
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
    )
END