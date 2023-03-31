CREATE OR REPLACE PROCEDURE  fox_document_read_binary_V1(_id uuid)
LANGUAGE SQL
BEGIN ATOMIC
	SELECT fileBinary FROM Document WHERE id = _id
END