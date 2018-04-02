<?php
require_once('includes/config.php');

//로그인하지 않았으면 로그인 페이지로 이동
if(!$user->is_logged_in())
{
	header("Location: index.php");
}
?>
<!DOCTYPE html>
<html lang="ko">
	<head>
		<title>멤버 페이지</title>
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
						<h2>멤버 전용 페이지 - 환영합니다. <?php echo $_SESSION["username"]; ?>님</h2>
						<p><a href="logout.php">로그아웃</a></p>
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
						<h1>hello world</h1>
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