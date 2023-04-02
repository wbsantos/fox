CREATE OR REPLACE PROCEDURE  fox_system_delpermission_v1 (
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM Stamp S
	USING SystemPermission P 
	WHERE
		 P.stampId = S.Id 
		 AND P.holderId = _holderId
		 AND P.permission = _permission;

	DELETE FROM SystemPermission
	WHERE
		holderId = _holderId
		AND permission = _permission;
END
$$;