<?php

require 'function.php';
$rows = getChannels();

echo json_encode([
	'table' => [
		'rows' => $rows,
	],
], JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
