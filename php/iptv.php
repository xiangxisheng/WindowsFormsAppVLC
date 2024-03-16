<?php

require 'function.php';
$channels = getChannels();
function items($rows, $cols)
{
	global $channels;
	// 计算出视频的数量
	$channel_count = count($channels);
	// 计算出每页能放几个视频
	$pagesize = $rows * $cols;
	// 计算出视频总共几页
	$pagetotal = ceil($channel_count / $pagesize);
	$ret = [];
	for ($i = 0; $i < $pagetotal; $i++) {
		$start = $i * $pagesize;
		$end = $start + $pagesize - 1;
		if ($end > $channel_count - 1) {
			$end = $channel_count - 1;
		}
		$ret[] = [
			'name' => '从 ' . ($start + 1) . ' 到 ' . ($end + 1),
			'rows' => $rows,
			'cols' => $cols,
			'start' => $start,
			'end' => $end,
		];
	}
	return $ret;
}
$menus = [
	[
		'name' => '4分屏',
		'items' => items(2, 2),
	],
	[
		'name' => '9分屏',
		'items' => items(3, 3),
	],
	[
		'name' => '16分屏',
		'items' => items(4, 4),
	],
	[
		'name' => '25分屏',
		'items' => items(5, 5),
	],
	[
		'name' => '36分屏',
		'items' => items(6, 6),
	],
];
if (version_compare($_SERVER['HTTP_VERSION'], '2024.3.16') === -1) {
	exit(json_encode([
		'message' => '您当前软件版本过低',
	], JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE));
}
exit(json_encode([
	'title' => '华电西港IPTV视频监控',
	'menus' => $menus,
	'channels' => $channels,
	'message' => '配置文件下载成功',
], JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE));
