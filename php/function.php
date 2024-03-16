<?php

function buildUrl($parts)
{
	// 构建 URL
	$url = '';

	// 协议
	if (isset($parts['scheme'])) {
		$url .= $parts['scheme'] . '://';
	}

	// 用户名和密码
	if (isset($parts['user'])) {
		$url .= $parts['user'];
		if (isset($parts['pass'])) {
			$url .= ':' . $parts['pass'];
		}
		$url .= '@';
	}

	// 主机
	if (isset($parts['host'])) {
		$url .= $parts['host'];
	}

	// 端口
	if (isset($parts['port'])) {
		$url .= ':' . $parts['port'];
	}

	// 路径
	if (isset($parts['path'])) {
		$url .= $parts['path'];
	}

	// 查询字符串
	if (isset($parts['query'])) {
		$url .= '?' . $parts['query'];
	}

	// 片段标识符
	if (isset($parts['fragment'])) {
		$url .= '#' . $parts['fragment'];
	}

	return $url;
}

function getChannels()
{
	$dsn = 'mysql:dbname=sdb30;host:127.0.0.1';
	$db = new PDO($dsn, 'root', '');
	$db->beginTransaction();
	$sql = 'SELECT t1.channelid,t1.channelnumber,t1.name title_cn,t2.name title_en,t1.liveurl,t1.isvisible FROM channel t1 LEFT JOIN channellangmap t2 ON t1.channelid=t2.channelid WHERE t1.isvisible=1';
	$stmt = $db->prepare($sql);
	$param = [];
	$stmt->execute($param);
	$db->rollBack();
	$rows = $stmt->fetchAll(PDO::FETCH_ASSOC);
	foreach ($rows as &$row) {
		if ($row['liveurl']) {
			$url = parse_url($row['liveurl']);
			$url['host'] = parse_url($_SERVER['HTTP_HOST'])['host'];
			$row['liveurl'] = buildUrl($url);
		}
	}
	return $rows;
}
