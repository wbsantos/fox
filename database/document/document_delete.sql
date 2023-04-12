create or replace procedure fox_document_delete_v1(_id uuid)
language plpgsql as
$$
declare _stamp_ids_documents int[];
declare _stamp_ids_permission int[];
begin
	_stamp_ids_documents := array(select stamp_id from document d where d.id = _id);
	_stamp_ids_permission := array(select stamp_id from document_permission where document_id = _id);

	delete from document_permission where document_id = _id;
	delete from document_metadata where document_id = _id;
	delete from document where id = _id;
	
	delete from stamp s where s.id = any(_stamp_ids_documents) and not exists(select 1 from document where stamp_id = s.id);
	delete from stamp s where s.id = any(_stamp_ids_permission) and not exists(select 1 from document_permission where stamp_id = s.id);
end
$$;