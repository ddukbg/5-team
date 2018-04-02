<?php
require_once('includes/config.php');

$username = $_POST['username'];
$password = $_POST['password'];
$securitycode = $_POST['securitycode'];
$useragent = $_SERVER['HTTP_USER_AGENT'];

$nowTime = date("YmdHi");
$nowTime2 = substr($nowTime, 0, strlen($nowTime) - 1);

$securitycodeShouldBe = hash('sha512', $nowTime2.$username.'login', false);
$useragentOK = false;

switch ($useragent)
{
	case "HypnotikAuthSystem-DEBUG":
		echo 'username: '.$_POST['username'];
		echo 'password: '.$_POST['password'];
		echo 'securitycode: '.$securitycode;
		echo 'useragent: '.$useragent;
		echo 'serverTime: '.$nowTime;
		echo 'serverTime2: '.$nowTime2;
		echo 'securitycodeShouldBe: '.$securitycodeShouldBe;
		$useragentOK = true;
	break;
	case "HypnotikAuthSystem":
		$useragentOK = true;
	break;
	default:
		$useragentOK = false;
	break;
}

if ($useragentOK == true &&
	!empty($securitycode) &&
	!empty($username) &&
	!empty($password) &&
	$securitycode == $securitycodeShouldBe &&
	$user->login($username, $password))
{
	//로그인 성공
	echo '<answer="'.hash('sha512', $nowTime2.$username.'alright', false).'">'; 
}
else
{
	//로그인 실패
	echo '<answer="'.hash('sha512', $nowTime2.$username.'wrong', false).'">'; 
}
?>