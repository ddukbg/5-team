<?php  

$link=mysqli_connect("localhost","interface518","518interface","interface518" ); 
 
if (!$link)  
{  
    echo "MySQL ���� ���� : ";
    echo mysqli_connect_error();
    exit();  
}  

mysqli_set_charset($link,"utf8"); 

$sql="select * FROM `AD` ORDER BY `id` DESC";

$result=mysqli_query($link,$sql);
$data = array();   

if($result){  
    
    while($row=mysqli_fetch_array($result)){
        array_push($data,
            array('contents'=>$row[0],'today'=>$row[1]));
    }

    header('Content-Type: application/json; charset=utf8');
$json = json_encode(array("webnautes"=>$data), JSON_PRETTY_PRINT+JSON_UNESCAPED_UNICODE);
echo $json;

}  
else{  
    echo "SQL�� ó���� ���� �߻� : "; 
    echo mysqli_error($link);
} 

mysqli_close($link);  
   