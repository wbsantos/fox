CREATE OR REPLACE PROCEDURE fox_document_addpermission_v1 (
			_stamp_id int,
			_document_id uuid,
			_holder_id uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	INSERT INTO document_permission (stamp_id, document_id, holder_id, permission)
	VALUES (_stamp_id, _document_id, _holder_id, _permission);
END
$$;