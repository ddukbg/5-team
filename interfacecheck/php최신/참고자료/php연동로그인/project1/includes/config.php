<?php
ob_start();
session_start();

//set timezone
date_default_timezone_set('UTC');

//database credentials
define('DBHOST','secret');
define('DBUSER','secret');
define('DBPASS','secret');
define('DBNAME','secret');

//application address
define('DIR','https://yourdomain.com');
define('SITEEMAIL','noreply@yourdomain.com');

try 
{
	//create PDO connection
	$db = new PDO("mysql:host=".DBHOST.";dbname=".DBNAME, DBUSER, DBPASS);
	$db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
}
catch(PDOException $e)
{
	//show error
    echo $e->getMessage();
    exit;
}

//include the user class, pass in the database connection
include('classes/user.php');
include('classes/phpmailer/mail.php');
$user = new User($db);
?>
