<?php  
error_reporting(E_ALL); 
ini_set('display_errors',1); 

$link=mysqli_connect("localhost","interface518","518interface","interface518" ); 
if (!$link)  
{  
    echo "MySQL ���� ���� : ";
    echo mysqli_connect_error();
    exit();  
}  

mysqli_set_charset($link,"utf-8"); 


//POST ���� �о�´�.
$id=isset($_POST['id']) ? $_POST['id'] : '';



if ($id !="" ){ 

    $sql="select * from Person where id='$id'";

    $result=mysqli_query($link,$sql);
    $data = array();   
    if($result){  
    
        $row_count = mysqli_num_rows($result);

        if ( 0 == $row_count ){
            echo "'";
            echo $id;
            echo "'�� ã�� �� �����ϴ�.";
        }
        else{

            while($row=mysqli_fetch_array($result)){
                array_push($data, 
                    array('id'=>$row[0],
                    'name'=>$row[1],
                    'address'=>$row[2],
	            'department'=>$row[3]
                ));
            }


            $android = strpos($_SERVER['HTTP_USER_AGENT'], "Android");

            if (!$android) {
                echo "<pre>"; 
                print_r($data); 
                echo '</pre>';
            }else
            {
                header('Content-Type: application/json; charset=utf8');
                $json = json_encode(array("webnautes"=>$data), JSON_PRETTY_PRINT+JSON_UNESCAPED_UNICODE);
                echo $json;
            }
        }


        mysqli_free_result($result);

    }
    else{  
        echo "SQL�� ó���� ���� �߻� : "; 
        echo mysqli_error($link);
    }
}
else {
    echo "�˻��� �й��� �Է��Ͻÿ�";
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
         �й�: <input type = "text" name = "id" />
         <input type = "submit" />
      </form>
   
   </body>
</html>
<?php
}

   
?>