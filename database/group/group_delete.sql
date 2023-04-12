create or replace procedure fox_group_delete_v1(_id uuid)
language plpgsql as
$$
declare _stamp_ids_docs int[];
declare _stamp_ids_system int[];
declare _stamp_ids_users int[];
begin
	_stamp_ids_docs := array(select stamp_id from document_permission where holder_id = _id);
	_stamp_ids_system := array(select stamp_id from system_permission where holder_id = _id);
	_stamp_ids_users := array(select stamp_id from user_group where group_id = _id);

	delete from document_permission where holder_id = _id;
	delete from system_permission where holder_id = _id;
	delete from user_group where group_id = _id;

	delete from group_account where id = _id;
	delete from holder where id = _id;

	delete from stamp s where s.id = any(_stamp_ids_docs) and not exists(select 1 from document_permission where stamp_id = s.id);
	delete from stamp s where s.id = any(_stamp_ids_system) and not exists(select 1 from system_permission where stamp_id = s.id);
	delete from stamp s where s.id = any(_stamp_ids_users) and not exists(select 1 from user_group where stamp_id = s.id);

end
$$;