create or replace procedure fox_user_delete_v1(_id uuid)
language plpgsql as
$$
begin
	delete from document_permission where holder_id = _id;
	delete from system_permission where holder_id = _id;
	delete from user_group where user_id = _id;

	delete from stamp where user_id = _id;
	delete from user_account where id = _id;
	delete from holder where id = _id;
end
$$;