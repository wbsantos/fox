CREATE OR REPLACE FUNCTION  fox_document_read_binary_v1(_id uuid)
RETURNS bytea
LANGUAGE plpgsql AS
$$
BEGIN
	SELECT fileBinary FROM Document WHERE id = _id;
END
$$;