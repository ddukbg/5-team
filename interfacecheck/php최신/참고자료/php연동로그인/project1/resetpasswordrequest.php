<?php
require_once("includes/config.php");

if($user->is_logged_in())
{
	header("Location: memberpage.php");
	exit;
}

//if form has been submitted process it
if(isset($_POST['submit']))
{
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

		if(empty($row['email']))
		{
			$error[] = '입력하신 이메일을 사용하는 계정이 없는 것 같습니다.';
		}
	}

	//if no errors have been created carry on
	if(!isset($error))
	{
		//create the activasion code
		$token = hash('sha512', uniqid(rand(), true));

		try
		{
			$stmt = $db->prepare("UPDATE members SET resetToken = :token, resetComplete = :resetComplete WHERE email = :email");
			$stmt->execute(array(
				':email' => $row['email'],
				':token' => $token,
				':resetComplete' => FALSE,
			));

			//send email
			$to = $row['email'];
			$subject = "비밀번호 초기화";
			$subject = "=?UTF-8?B?".base64_encode($subject)."?=";//한글깨짐 패치
			$body = "<a href='".DIR."resetpassword.php?key=$token'>".DIR."resetPassword.php?key=$token</a>";

			$mail = new Mail();
			$mail->setFrom(SITEEMAIL);
			$mail->addAddress($to);
			$mail->subject($subject);
			$mail->body($body);
			$mail->send();

			//redirect to index page
			header('Location: index.php?action=reset');
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
		<title>비밀번호 초기화</title>
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
						<h2>비밀번호 초기화</h2>
						<p><a href="index.php">로그인 페이지로 돌아가기</a></p>
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
							<input type="email" name="email" id="email" class="form-control input-lg" placeholder="이메일 주소" value="" tabindex="1">
						</div>
						<hr>
						<div class="row">
							<div class="col-xs-6 col-md-6"><input type="submit" name="submit" value="요청" class="btn btn-primary btn-block btn-lg" tabindex="2"></div>
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
