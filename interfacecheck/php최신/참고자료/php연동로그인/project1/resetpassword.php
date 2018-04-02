<?php
require_once('includes/config.php');

if($user->is_logged_in())
{
	header('Location: memberpage.php');
	exit;
}

if(isset($_GET['key']))
{
	$stmt = $db->prepare('SELECT resetToken, resetComplete FROM members WHERE resetToken = :token');
	$stmt->execute(array(':token' => $_GET['key']));
	$row = $stmt->fetch(PDO::FETCH_ASSOC);

	if(empty($row['resetToken']))
	{
		$stop = '유효하지 않은 토큰입니다.';
	}
	elseif($row['resetComplete'] == TRUE)
	{
		$stop = '비밀번호가 이미 변경되었습니다!';
	}
}

if(isset($_POST['submit']))
{
	//basic validation
	if(strlen($_POST['password']) < 6)
	{
		$error[] = '비밀번호가 너무 짧습니다.';
	}

	if($_POST['password'] != $_POST['passwordConfirm'])
	{
		$error[] = '확인 비밀번호가 일치하지 않습니다.';
	}

	//if no errors have been created carry on
	if(!isset($error))
	{
		//hash the password
		$hashedpassword = $user->password_hash($_POST['password'], PASSWORD_BCRYPT);

		try
		{
			$stmt = $db->prepare("UPDATE members SET password = :newPassword, resetComplete = :newResetComplete, resetToken = :newResetToken WHERE resetToken = :resetToken");
			$stmt->execute(array(
				':newPassword' => $hashedpassword,
				':resetToken' => $row['resetToken'],
				':newResetComplete' => TRUE,
				':newResetToken' => NULL,
			));

			//redirect to index page
			header('Location: index.php?action=resetAccount');
			exit;
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
		<title>비밀번호 변경</title>
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
					<?php
					if(isset($stop))
					{
						echo "<p class='bg-danger'>".$stop."</p>";
					}
					else
					{?>
						<form role="form" method="post" action="" autocomplete="off">
							<h2>비밀번호 변경</h2>
							<p><a href='index.php'>로그인 페이지로 돌아가기</a></p>
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
							<div class="row">
								<div class="col-xs-6 col-sm-6 col-md-6">
									<div class="form-group">
										<input type="password" name="password" id="password" class="form-control input-lg" placeholder="비밀번호" tabindex="1">
									</div>
								</div>
								<div class="col-xs-6 col-sm-6 col-md-6">
									<div class="form-group">
										<input type="password" name="passwordConfirm" id="passwordConfirm" class="form-control input-lg" placeholder="비밀번호 재입력" tabindex="2">
									</div>
								</div>
							</div>
							<hr>
							<div class="row">
								<div class="col-xs-6 col-md-6"><input type="submit" name="submit" value="변경" class="btn btn-primary btn-block btn-lg" tabindex="3"></div>
							</div>
						</form>
					<?php } ?>
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