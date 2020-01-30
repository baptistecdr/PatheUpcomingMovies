<?php
if(filter_has_var(INPUT_GET, "v")) {
    $id = filter_input(INPUT_GET, "v", FILTER_SANITIZE_NUMBER_INT);
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, 'https://player.vimeo.com/video/' . $id);
    curl_setopt($ch, CURLOPT_USERAGENT, 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:72.0) Gecko/20100101 Firefox/72.0');
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($ch, CURLOPT_REFERER, 'https://pathe.ch/');
    echo curl_exec($ch);
}
?>
