<?php  
error_reporting(E_ALL); 
ini_set('display_errors',1); 

$link=mysqli_connect("localhost","interface518","518interface","interface518"); 
if (!$link)  
{ 
   echo "MySQL ���� ���� : ";
   echo mysqli_connect_error();
   exit();
}  


mysqli_set_charset($link,"utf-8");  

//POST ���� �о�´�.
$contents=isset($_POST['contents']) ? $_POST['contents'] : '';  


if ($contents !=""){   

    $today = date("n�� j�� h�� i��");
    $sql="insert into AD(contents, today) values('$contents', '$today')";  
    $result=mysqli_query($link,$sql);  

    if($result){  
       echo "SQL�� ó�� ����";  
    }  
    else{  
       echo "SQL�� ó���� ���� �߻� : "; 
       echo mysqli_error($link);
    } 
 
} else {
    echo "�����͸� �Է��ϼ��� ";
}


mysqli_close($link);
?>

<?php

$android = strpos($_SERVER['HTTP_USER_AGENT'], "Android");

if (!$android){
?>

<html>
   <body>
   
      <form action="<?php $_PHP_SELF ?>" method="POST">
         �������� : <input type = "text" name = "contents" />
         <input type = "submit" />
      </form>
   
   </body>
</html>
<?php
}
?>
