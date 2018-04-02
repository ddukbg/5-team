<?php
require_once("includes/config.php");

if($user->is_logged_in())
{
	header("Location: memberpage.php");
	exit;
}

if(isset($_POST["submit"]))
{
	$username = $_POST["username"];
	$password = $_POST["password"];

	if($user->login($username, $password))
	{
		header("Location: memberpage.php");
		exit;
	}
	else
	{
		$error[] = "아이디나 비밀번호가 틀렸거나, 계정이 활성화되지 않았습니다.";
	}
}
?>
<!DOCTYPE html>
<html lang="ko">
	<head>
		<title>로그인</title>
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
						<h2>로그인 해주세요.</h2>
						<p>아직 계정이 없으신가요? <a href="register.php">회원가입</a></p>
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

						if(isset($_GET['action']))
						{
							switch ($_GET["action"])
							{
								case "active":
									echo "<h2 class='bg-success'>계정이 활성화되었습니다, 바로 로그인 가능합니다.</h2>";
									break;
								case "reset":
									echo "<h2 class='bg-success'>비밀번호 초기화 링크를 보냈습니다, 메일함을 확인해주세요.</h2>";
									break;
								case "resetAccount":
									echo "<h2 class='bg-success'>비밀번호가 변경되었습니다, 바로 로그인가능합니다.</h2>";
									break;
								case "joined":
									echo "<h2 class='bg-success'>회원가입이 완료되었습니다, 이메일 인증을 완료하면 계정이 활성화됩니다.</h2>";
									break;
							}
						}
						?>
						<div class="form-group">
							<input type="text" name="username" class="form-control input-lg" placeholder="아이디" value="<?php if(isset($error)){ echo $_POST["username"]; } ?>" tabindex="1">
						</div>
						<div class="form-group">
							<input type="password" name="password" class="form-control input-lg" placeholder="비밀번호" value="" tabindex="2">
						</div>
						<div class="row">
							<div class="col-xs-9 col-sm-9 col-md-9">
								<a href="resetpasswordrequest.php">비밀번호를 잊으셨나요?</a>
							</div>
						</div>
						<hr>
						<div class="row">
							<div class="col-xs-6 col-sm-6 col-md-6">
								<input type="submit" name="submit" value="로그인" class="btn btn-primary btn-block btn-lg" tabindex="3">
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