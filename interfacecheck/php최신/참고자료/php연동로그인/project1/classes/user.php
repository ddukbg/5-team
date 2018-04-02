﻿<?php
include('password.php');

class User extends Password
{
    private $_db;

    function __construct($db)
	{
    	parent::__construct();

    	$this->_db = $db;
    }

	private function get_user_hash($username)
	{
		try
		{
			$stmt = $this->_db->prepare('SELECT joindatetime, password, username, memberID FROM members WHERE username = :username AND active = :active');
			$stmt->execute(array(
				'username' => $username,
				'active' => TRUE,
				));

			return $stmt->fetch();

		}
		catch(PDOException $e)
		{
		    echo '<p class="bg-danger">'.$e->getMessage().'</p>';
		}
	}

	public function login($username,$password)
	{
		$row = $this->get_user_hash($username);

		if($this->password_verify($password,$row['password']) == TRUE)
		{
		    $_SESSION['loggedin'] = true;
		    $_SESSION['username'] = $row['username'];
		    $_SESSION['memberID'] = $row['memberID'];
			$_SESSION['joindatetime'] = $row['joindatetime'];
		    return true;
		}
	}

	public function logout()
	{
		session_destroy();
	}

	public function is_logged_in()
	{
		if(isset($_SESSION['loggedin']) && $_SESSION['loggedin'] == true)
		{
			return true;
		}
	}
}
?>
