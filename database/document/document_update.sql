CREATE OR REPLACE PROCEDURE  fox_document_update_V1(
			_id uuid,
			_name varchar(255))
LANGUAGE SQL
BEGIN ATOMIC
	UPDATE Document
	SET
		name = _name
	WHERE
		id = _id
END