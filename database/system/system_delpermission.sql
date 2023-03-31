CREATE OR REPLACE PROCEDURE  fox_system_delpermission_V1 (
			_holderId uuid,
			_permission varchar(255)
)
LANGUAGE SQL
BEGIN ATOMIC
	DELETE S FROM Stamp 
	JOIN SystemPermission P ON P.stampId = S.Id 
	WHERE
		 P.holderId = _holderId
		AND P.permission = _permission

	DELETE FROM SystemPermission
	WHERE
		holderId = _holderId
		AND permission = _permission
END