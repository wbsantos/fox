create or replace procedure fox_document_delete_v1(_id uuid)
language plpgsql as
$$
begin
	delete from document_permission where document_id = _id;
	delete from document_metadata where document_id = _id;
	delete from document where id = _id;
end
$$;