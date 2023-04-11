create or replace procedure fox_document_addpermission_v1 (
			_stamp_id int,
			_document_id uuid,
			_holder_id uuid,
			_permission varchar(255)
)
language plpgsql as
$$
begin
	insert into document_permission (stamp_id, document_id, holder_id, permission)
	values (_stamp_id, _document_id, _holder_id, _permission);
end
$$;