<?php
require_once('includes/config.php');

//collect values from the url
$memberID = trim($_GET['x']);
$activeToken = trim($_GET['y']);

//if id is number and the active token is not empty carry on
if(is_numeric($memberID) && !empty($activeToken))
{
	//update users record set the active column to Yes where the memberID and active value match the ones provided in the array
	$stmt = $db->prepare("UPDATE members SET active = :newActive, activeToken = :newActiveToken WHERE memberID = :memberID AND activeToken = :activeToken");
	$stmt->execute(array(
		':memberID' => $memberID,
		':activeToken' => $activeToken,
		':newActive' => TRUE,
		':newActiveToken' => NULL,
	));

	//if the row was updated redirect the user
	if($stmt->rowCount() == 1)
	{
		//redirect to index page
		header('Location: index.php?action=active');
		exit;
	}
	else
	{
		echo "계정을 활성화하지 못했습니다.";
	}
}
?>