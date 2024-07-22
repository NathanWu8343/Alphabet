import http from 'k6/http';
import { sleep, check, group } from 'k6';

const host = __ENV.HOST || 'https://localhost:7059'

//export let options = {
//    scenarios: {
//        constant_request_rate: {
//            executor: "constant-arrival-rate",
//            rate: 1,
//            timeUnit: "15s", // 1 iterations per 15 second
//            duration: "1m",
//            preAllocatedVUs: 5, // how large the initial pool of VUs would be
//        },
//    },
//};

//export let options = {
//    scenarios: {
//        registration: {
//            executor: 'per-vu-iterations',
//            vus: 100,
//            iterations: 100,
//            maxDuration: '300s',
//        },
//    }
//};

//const stages = [];
//for (let t = 50; t <= 700; t += 50) {
//    stages.push({ duration: '5s', target: t }, { duration: '5s', target: t });
//}
//export let options = {
//    systemTags: ['status', 'error', 'vu', 'iter'],
//    scenarios: {
//        stress: {
//            executor: 'ramping-arrival-rate',
//            preAllocatedVUs: 10000,
//            timeUnit: "1s",
//            stages: stages
//        }
//    }
//};

//export let options = {
//    vus: 5, // 并发虚拟用户数
//    duration: '1m', // 测试持续时间
//    // 设置速率限制
//    thresholds: {
//        http_req_duration: ['p(99)<2000'], // 95% 的请求响应时间在 2 秒以内
//    },
//    rps: 100, // 每秒请求数
//};

// Test configuration
//export const options = {
//    thresholds: {
//        // Assert that 99% of requests finish within 3000ms.
//        http_req_duration: ["p(99) < 3000"],
//    },
//    // Ramp the number of virtual users up and down
//    stages: [
//        { duration: "30s", target: 15 },
//        { duration: "1m", target: 15 },
//        { duration: "20s", target: 0 },
//    ],
//};

//export const options = {
//    discardResponseBodies: true,
//    scenarios: {
//        contacts: {
//            executor: 'shared-iterations',
//            vus: 5,
//            iterations: 10,
//            maxDuration: '30s',
//        },
//    },
//};

//export const options = {
//    scenarios: {
//        shared_iter_scenario: {
//            executor: 'shared-iterations',
//            vus: 10,
//            iterations: 100,
//            startTime: '0s',
//        },
//        per_vu_scenario: {
//            executor: 'per-vu-iterations',
//            vus: 10,
//            iterations: 10,
//            startTime: '10s',
//        },
//    },
//};

//export const options = {
//    scenarios: {
//        constant_load: {
//            executor: 'constant-arrival-rate',
//            preAllocatedVUs: 100,
//            rate: 2,
//            timeUnit: '10s',
//            duration: "30s",
//        },
//    },
//};

//export const options = {
//    vus: 10,
//    iterations: 10,
//    //summaryTrendStats: ['avg', 'min', 'med', 'max', 'p(95)', 'p(99)', 'p(99.99)', 'count'],
//};

//export const options = {
//    scenarios: {
//        closed_model: {
//            executor: 'constant-vus',
//            vus: 1,
//            duration: '1m',
//        },
//        open_model: {
//            executor: 'constant-arrival-rate',
//            rate: 1,
//            timeUnit: '1s',
//            duration: '1m',
//            preAllocatedVUs: 20,
//        },
//    },
//};

export const options = {
    discardResponseBodies: true,
    scenarios: {
        contacts: {
            executor: 'constant-arrival-rate',

            // How long the test lasts
            duration: '30s',

            // How many iterations per timeUnit
            rate: 30,

            // Start `rate` iterations per second
            timeUnit: '1s',

            // Pre-allocate 2 VUs before starting the test
            preAllocatedVUs: 2,

            // Spin up a maximum of 50 VUs to sustain the defined
            // constant arrival rate.
            maxVUs: 50,
        },
    },
};
const params = {
    headers: {
        'Content-Type': 'application/json'
    }
}

export default function () {
    group('visit product listing page', function () {
        const payload = JSON.stringify({
            'url': 'http://google.com'
        });

        const res = http.post(`${host}/url-shortener-api/v1/shorten/create`, payload, params);
        check(res, { 'status was 200': (r) => r.status === 200 });
    });
}