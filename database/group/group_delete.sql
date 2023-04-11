create or replace procedure fox_group_delete_v1(_id uuid)
language plpgsql as
$$
begin
	delete from document_permission where holder_id = _id;
	delete from system_permission where holder_id = _id;
	delete from user_group where group_id = _id;

	delete from group_account where id = _id;
	delete from holder where id = _id;
end
$$;