<?php
require_once("includes/config.php");

if($user->is_logged_in())
{
	header("Location: memberpage.php");
	exit;
}

if(isset($_POST["submit"]))
{
	if(strlen($_POST["username"]) < 3)
	{
		$error[] = "아이디가 너무 짧습니다.";
	}
	else
	{
		$stmt = $db->prepare('SELECT username FROM members WHERE username = :username');
		$stmt->execute(array(':username' => $_POST['username']));
		$row = $stmt->fetch(PDO::FETCH_ASSOC);

		if(!empty($row['username']))
		{
			$error[] = '입력하신 아이디가 이미 사용중입니다.';
		}
	}

	if(strlen($_POST['password']) < 6)
	{
		$error[] = '비밀번호가 너무 짧습니다.';
	}

	if($_POST['password'] != $_POST['passwordConfirm'])
	{
		$error[] = '비밀번호가 일치하지 않습니다.';
	}

	//email validation
	if(!filter_var($_POST['email'], FILTER_VALIDATE_EMAIL))
	{
		$error[] = '유효한 이메일 주소를 입력해주세요.';
	}
	else
	{
		$stmt = $db->prepare('SELECT email FROM members WHERE email = :email');
		$stmt->execute(array(':email' => $_POST['email']));
		$row = $stmt->fetch(PDO::FETCH_ASSOC);

		if(!empty($row['email']))
		{
			$error[] = '입력하신 이메일이 이미 사용중입니다.';
		}
	}

	//if no errors have been created carry on
	if(!isset($error))
	{
		//hash the password
		$hashedpassword = $user->password_hash($_POST['password'], PASSWORD_BCRYPT);
		//create the activasion code
		$activasion = hash('sha512', uniqid(rand(), true));
		try
		{
			//insert into database with a prepared statement
			$stmt = $db->prepare('INSERT INTO members (joindatetime, username, password, email, active, activeToken) VALUES (:joindatetime, :username, :password, :email, :active, :activeToken)');
			$stmt->execute(array(
				':joindatetime' => date("Y-m-d H:i:s"),
				':username' => $_POST['username'],
				':password' => $hashedpassword,
				':email' => $_POST['email'],
				':active' => FALSE,
				':activeToken' => $activasion,
			));
			$id = $db->lastInsertId('memberID');
			//send email
			$to = $_POST['email'];
			$subject = "회원가입 이메일 인증";
			$subject = "=?UTF-8?B?".base64_encode($subject)."?=";//한글깨짐 패치
			$body = "<a href='".DIR."activate.php?x=$id&y=$activasion'>".DIR."activate.php?x=$id&y=$activasion</a>";
			$mail = new Mail();
			$mail->setFrom(SITEEMAIL);
			$mail->addAddress($to);
			$mail->subject($subject);
			$mail->body($body);
			$mail->send();
			//redirect to index page
			header('Location: index.php?action=joined');
			exit;
			//else catch the exception and show the error.
		}
		catch(PDOException $e)
		{
			$error[] = $e->getMessage();
		}
	}
}
?>
<!DOCTYPE html>
<html lang="ko">
	<head>
		<title>회원가입</title>
		<meta charset="UTF-8">
		<meta name="viewport" content="width=device-width, initial-scale=1">
		<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
	</head>
	<body>
		<?php
		require("layout/header.php");
		?>

		<div class="container">
			<div class="row">
				<div class="col-xs-12 col-sm-8 col-md-6 col-sm-offset-2 col-md-offset-3">
					<form role="form" method="post" action="" autocomplete="off">
						<h2>회원가입 해주세요.</h2>
						<p>이미 회원이신가요? <a href="index.php">로그인</a></p>
						<hr>
						<?php
						//check for any errors
						if(isset($error))
						{
							foreach($error as $error)
							{
								echo '<p class="bg-danger">'.$error.'</p>';
							}
						}
						?>
						<div class="form-group">
							<input type="text" name="username" class="form-control input-lg" placeholder="아이디" value="" tabindex="1">
						</div>
						<div class="form-group">
							<input type="email" name="email" class="form-control input-lg" placeholder="이메일 주소", value="", tabindex="2">
						</div>
						<div class="row">
							<div class="form-group col-xs-12 col-sm-6 col-md-6">
								<input type="password" name="password" class="form-control input-lg" placeholder="비밀번호" value="" tabindex="3">
							</div>
							<div class="form-group col-xs-12 col-sm-6 col-md-6">
								<input type="password" name="passwordConfirm" class="form-control input-lg" placeholder="비밀번호 확인" value="" tabindex="4">
							</div>
						</div>
						<hr>
						<div class="row">
							<div class="col-xs-6 col-sm-6 col-md-6">
								<input type="submit" name="submit" value="가입" class="btn btn-primary btn-block btn-lg" tabindex="5">
							</div>
						</div>
					</form>
				</div>
			</div>
		</div>

		<?php
		require("layout/footer.php");
		?>

		<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
		<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
	</body>
</html>